import React, { useEffect, useState } from "react";
import styles from "./index.module.css";
import dayjs from "dayjs";
import { notification } from "antd";
import { apiClient } from "../../../helper/apiHelper";
export default function SettlementPage({ accessToken }) {
   const [selectedMenu, setSelectedMenu] = useState("");
   const [selectedTypeAsset, setSelectedTypeAsset] = useState("D");
   const [listMarket, setListMarket] = useState([]);
   const [batchData, setBatchData] = useState({});
   const [selectedItems, setSelectedItems] = useState({});
   const handleMenuClick = (menuKey) => {
      setSelectedMenu(menuKey);
      setSelectedItems({});
      getMemberSettle(menuKey);
   };

   const handleCheckboxChange = (code, checked) => {
      setSelectedItems((prev) => ({
         ...prev,
         [code]: checked,
      }));
   };

   const handleSelectAll = (checked) => {
      const currentRows = batchData[selectedMenu] || [];
      const filteredRows = currentRows.filter((row) => row.debit_Credit === selectedTypeAsset);

      const newSelected = {};
      if (checked) {
         filteredRows.forEach((row) => {
            newSelected[row.settlementId] = true;
         });
      }
      setSelectedItems(newSelected);
   };

   const currentRows = batchData[selectedMenu] || [];
   const filteredRows = currentRows.filter((row) => row.debit_Credit === selectedTypeAsset);
   const allSelected = filteredRows.length > 0 && filteredRows.every((row) => selectedItems[row.settlementId]);

   const getMarketList = () => {
      const data = apiClient.get(`/api/settling/get-market`);
      data
         .then((json) => {
            setSelectedMenu(json[0].marketId);
            getMemberSettle(json[0].marketId);
            setListMarket(json);
         })
         .catch((err) => console.error(err));
   };
   const getMemberSettle = (marketId) => {
      const data = apiClient.get(`/api/settling/get-member-settlement?marketId=${marketId}`);
      data
         .then((json) => {
            setBatchData({ [marketId]: json });
            const initSelected = {};
            json.forEach((row) => {
               initSelected[row.settlementId] = true;
            });
            setSelectedItems(initSelected);
         })
         .catch((err) => console.error(err));
   };
   const paymentSettle = (marketId, typeAsset) => {
      const data = apiClient.get(`/api/settling/run?marketId=${marketId}&debitCredit=${typeAsset}`);
      data
         .then((json) => {
            notification.success({
               message: "Thông báo",
               description: "Thanh toán đã được xử lý thành công.",
            });
            getMemberSettle(marketId);
         })
         .catch((err) => {
            notification.error({
               message: "Thông báo",
               description: "Có lỗi xảy ra trong quá trình xử lý thanh toán.",
            });
         });
   };
   useEffect(() => {
      getMarketList();
   }, []);
   return (
      <div className={styles.settleContainer}>
         <div className={styles.settleLeft}>
            <div className={styles.settleTopHeader}>
               <h3>Settlement Service</h3>
            </div>
            <div className={styles.settleMenu}>
               <ul className={styles.settleMenuList}>
                  {listMarket.map((market) => (
                     <li key={market.marketId} className={styles.settleMenuItem}>
                        <a
                           href="#"
                           className={`${styles.settleMenuLink} ${selectedMenu === market.marketId ? styles.settleMenuLinkActive : ""}`}
                           onClick={(e) => {
                              e.preventDefault();
                              handleMenuClick(market.marketId);
                           }}
                        >
                           {market.displayName}
                        </a>
                     </li>
                  ))}
               </ul>
            </div>
         </div>
         <div className={styles.settleRight}>
            <div className={styles.settleRightHeader}>
               <div className={styles.title}>{listMarket.find((market) => market.marketId === selectedMenu)?.displayName}</div>

               <div className={styles.listButton}>
                  <button className={`${styles.button} ${styles.payment}`} onClick={() => paymentSettle(selectedMenu, selectedTypeAsset)}>
                     Thanh toán
                  </button>
                  <button className={`${styles.button} ${styles.add}`}>Thêm mới</button>
               </div>
            </div>
            <div className={styles.listTypeAsset}>
               <button className={`${styles.button} ${selectedTypeAsset === "D" ? styles.active : ""}`} onClick={() => setSelectedTypeAsset("D")}>
                  Debit
               </button>
               <button className={`${styles.button} ${selectedTypeAsset === "C" ? styles.active : ""}`} onClick={() => setSelectedTypeAsset("C")}>
                  Credit
               </button>
            </div>

            <table className={styles.settleTable}>
               <thead>
                  <tr>
                     <th style={{ width: "30px", textAlign: "center" }}>
                        <input type="checkbox" className={styles.settleCheckbox} checked={allSelected} onChange={(e) => handleSelectAll(e.target.checked)} />
                     </th>
                     <th style={{ width: "200px" }}>Tên thành viên</th>
                     <th style={{ width: "200px" }}>Tài khoản chuyển</th>
                     <th style={{ width: "200px" }}>Tài khoản nhận</th>
                     <th style={{ width: "100px" }}>Giá trị</th>
                     <th style={{ width: "240px" }}>Nội dung</th>
                     <th style={{ width: "100px" }}>Trạng thái</th>
                     <th style={{ width: "100px" }}>Ngày</th>
                  </tr>
               </thead>
               <tbody>
                  {filteredRows.map((row) => (
                     <tr key={row.settlementId}>
                        <td style={{ textAlign: "center" }}>
                           <input
                              type="checkbox"
                              className={styles.settleCheckbox}
                              checked={selectedItems[row.settlementId] || false}
                              onChange={(e) => handleCheckboxChange(row.settlementId, e.target.checked)}
                           />
                        </td>
                        <td>{row.memberName}</td>
                        <td>{row.debitAccount}</td>
                        <td>{row.creditAccount}</td>
                        <td>{new Intl.NumberFormat("en-EN").format(row.amount)}</td>
                        <td>{row.description}</td>
                        <td>{row.statusText}</td>
                        <td>{dayjs(row.createDate).format("DD/MM/YYYY")}</td>
                     </tr>
                  ))}
               </tbody>
            </table>
         </div>
      </div>
   );
}
