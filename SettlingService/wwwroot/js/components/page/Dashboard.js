import React, { useState } from "react";
import { useNavigate } from "react-router-dom";

export default function Dashboard() {
   <></>;
   // const [selectedMenu, setSelectedMenu] = useState("SE");
   // const [currentDate, setCurrentDate] = useState(new Date().toISOString().split("T")[0]);
   // const [batchData, setBatchData] = useState({
   //    SE: [
   //       { Code: "SE01-CHECKRUNATBC", Name: "Kiểm tra hoàn thành nghiệp vụ BC", Status: "Chưa thực hiện", Desc: "-", Start: "-", End: "-" },
   //       { Code: "SE02-UPDATE_ORDERSTATUS", Name: "Cập nhật trạng thái lệnh", Status: "Chưa thực hiện", Desc: "-", Start: "-", End: "-" },
   //       { Code: "SE03-CALC_FEE_ORDER", Name: "Tính toán phí giao dịch, phí tài sản cuối ngày", Status: "Chưa thực hiện", Desc: "-", Start: "-", End: "-" },
   //    ],
   //    CP: [
   //       { Code: "CP01-LOAN_CHECK", Name: "Kiểm tra vay nợ tín dụng", Status: "Chưa thực hiện", Desc: "-", Start: "-", End: "-" },
   //       { Code: "CP02-LOAN_UPDATE", Name: "Cập nhật trạng thái vay nợ", Status: "Chưa thực hiện", Desc: "-", Start: "-", End: "-" },
   //    ],
   //    ED: [{ Code: "ED01-ENDDAY", Name: "Xử lý cuối ngày", Status: "Chưa thực hiện", Desc: "-", Start: "-", End: "-" }],
   // });
   // const [selectedItems, setSelectedItems] = useState({});
   // const [loading, setLoading] = useState(false);
   // const navigate = useNavigate();

   // const handleMenuClick = (menuKey) => {
   //    setSelectedMenu(menuKey);
   //    setSelectedItems({});
   // };

   // const handleCheckboxChange = (code, checked) => {
   //    setSelectedItems((prev) => ({
   //       ...prev,
   //       [code]: checked,
   //    }));
   // };

   // const handleSelectAll = (checked) => {
   //    const currentRows = batchData[selectedMenu] || [];
   //    const newSelected = {};
   //    if (checked) {
   //       currentRows.forEach((row) => {
   //          newSelected[row.Code] = true;
   //       });
   //    }
   //    setSelectedItems(newSelected);
   // };

   // const handleRunBatch = async () => {
   //    const selectedCodes = Object.keys(selectedItems).filter((code) => selectedItems[code]);
   //    if (selectedCodes.length === 0) {
   //       alert("Vui lòng chọn ít nhất một nghiệp vụ để chạy batch");
   //       return;
   //    }

   //    setLoading(true);
   //    try {
   //       const response = await fetch("/api/api/runBatch", {
   //          method: "POST",
   //          headers: { "Content-Type": "application/json" },
   //          body: JSON.stringify({ codes: selectedCodes, date: currentDate }),
   //       });
   //       const data = await response.json();
   //       console.log("✅ Response:", data);
   //       alert(data.Message || "Batch đã được chạy thành công");
   //    } catch (err) {
   //       alert("Có lỗi xảy ra khi chạy batch");
   //    } finally {
   //       setLoading(false);
   //    }
   // };

   // const currentRows = batchData[selectedMenu] || [];
   // const allSelected = currentRows.length > 0 && currentRows.every((row) => selectedItems[row.Code]);

   // return (
   //    <div className="card shadow-sm">
   //       <div className="card-body">
   //          <h3 className="mb-4">Loại giao dịch</h3>

   //          <div className="row">
   //             {/* Menu trái */}
   //             <div className="col-md-2 border-end">
   //                <ul className="nav flex-column">
   //                   <li className="nav-item mb-2">
   //                      <a
   //                         href="#"
   //                         className={`nav-link ${selectedMenu === "SE" ? "active text-primary" : "text-dark"} fw-semibold`}
   //                         onClick={(e) => {
   //                            e.preventDefault();
   //                            handleMenuClick("SE");
   //                         }}
   //                      >
   //                         Debit
   //                      </a>
   //                   </li>
   //                   <li className="nav-item mb-2">
   //                      <a
   //                         href="#"
   //                         className={`nav-link ${selectedMenu === "CP" ? "active text-primary" : "text-dark"} fw-semibold`}
   //                         onClick={(e) => {
   //                            e.preventDefault();
   //                            handleMenuClick("CP");
   //                         }}
   //                      >
   //                         Credit
   //                      </a>
   //                   </li>
   //                </ul>
   //             </div>
   //             <div className="col-md-9">
   //                <div className="d-flex align-items-center justify-between mb-3">
   //                   <div>Danh sách lệnh</div>
   //                   <div>
   //                      <button>Thanh toán</button>
   //                      <button>Thêm mới</button>
   //                   </div>
   //                </div>

   //                <table className="table table-bordered table-striped align-middle">
   //                   <thead className="table-light">
   //                      <tr>
   //                         <th scope="col">
   //                            <input type="checkbox" checked={allSelected} onChange={(e) => handleSelectAll(e.target.checked)} />
   //                         </th>
   //                         <th scope="col">Mã nghiệp vụ</th>
   //                         <th scope="col">Tên nghiệp vụ</th>
   //                         <th scope="col">Trạng thái</th>
   //                         <th scope="col">Mô tả</th>
   //                         <th scope="col">Thời gian bắt đầu</th>
   //                         <th scope="col">Thời gian kết thúc</th>
   //                      </tr>
   //                   </thead>
   //                   <tbody>
   //                      {currentRows.map((row) => (
   //                         <tr key={row.Code}>
   //                            <td>
   //                               <input type="checkbox" checked={selectedItems[row.Code] || false} onChange={(e) => handleCheckboxChange(row.Code, e.target.checked)} />
   //                            </td>
   //                            <td>{row.Code}</td>
   //                            <td>{row.Name}</td>
   //                            <td>{row.Status}</td>
   //                            <td>{row.Desc}</td>
   //                            <td>{row.Start}</td>
   //                            <td>{row.End}</td>
   //                         </tr>
   //                      ))}
   //                   </tbody>
   //                </table>
   //             </div>
   //          </div>
   //       </div>
   //    </div>
   // );
}
