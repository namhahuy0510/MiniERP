## HƯỚNG DẪN SỬ DỤNG ỨNG DỤNG MINI ERP

Ứng dụng **MiniERP** là một web app quản lý nhân sự và chấm công đơn giản, gồm các chức năng chính:
- **Đăng nhập / Đăng ký**
- **Quản lý nhân viên**
- **Chấm công (Attendance)**
- **Tự động tính lương tháng (tab Lương)**

---

## 1. TRUY CẬP ỨNG DỤNG

- Mở trình duyệt (Chrome, Edge, …).
- Truy cập vào địa chỉ mà ứng dụng được triển khai (ví dụ: `http://localhost:5000` hoặc địa chỉ do quản trị cung cấp).
- Giao diện chính sẽ hiển thị thanh menu trên cùng với các mục:
  - **Trang chủ**
  - **QL Nhân viên**
  - **Lương**
  - Góc phải là khu vực **Đăng ký / Đăng nhập** hoặc tên người dùng khi đã đăng nhập.

---

## 2. ĐĂNG KÝ TÀI KHOẢN

1. Trên thanh menu, bấm **“Đăng ký”**.
2. Điền các thông tin yêu cầu (Email, Mật khẩu, Xác nhận mật khẩu, …).
3. Bấm **Đăng ký**.
4. Sau khi đăng ký thành công, tùy cấu hình hệ thống, tài khoản có thể:
   - Được dùng ngay, hoặc
   - Cần Admin phân quyền thêm (ví dụ quyền **Admin** để quản lý nhân viên).

> Lưu ý: Chỉ tài khoản có quyền **Admin** mới được thêm / sửa / xóa nhân viên.

---

## 3. ĐĂNG NHẬP / ĐĂNG XUẤT

### 3.1. Đăng nhập

1. Trên thanh menu, bấm **“Đăng nhập”**.
2. Nhập **Email** và **Mật khẩu** đã đăng ký.
3. Bấm **Đăng nhập**.
4. Nếu đúng, góc phải sẽ hiển thị: “**Xin chào [Tên tài khoản]**” và nút **Đăng xuất**.

### 3.2. Đăng xuất

1. Trên thanh menu, bấm nút **“Đăng xuất”**.
2. Hệ thống sẽ đưa bạn về trạng thái khách (chưa đăng nhập).

---

## 4. QUẢN LÝ NHÂN VIÊN (QL NHÂN VIÊN)

Truy cập: trên menu, chọn **“QL Nhân viên”**.

### 4.1. Xem danh sách nhân viên

Màn hình **Trang quản lý nhân viên** hiển thị:
- Danh sách tất cả nhân viên với các cột:
  - **STT**
  - **Id**
  - **Họ tên**
  - **Ngày sinh**
  - **Vị trí**
  - **Lương** (mức lương cơ bản theo tháng)
  - **Ngày vào**
  - **Ngày kết thúc** (nếu có)
- Nếu bạn có quyền **Admin**, sẽ thấy các nút:
  - **Thêm nhân viên mới**
  - **Sửa**
  - **Xóa**

### 4.2. Tìm kiếm nhân viên

Trên đầu danh sách có ô **tìm kiếm**:
- Nhập từ khóa: **Tên nhân viên**, **Vị trí**, …
- Bấm nút **“Tìm kiếm”**.
- Hệ thống sẽ hiển thị danh sách nhân viên phù hợp.

### 4.3. Thêm nhân viên mới (chỉ Admin)

1. Ở trang **Trang quản lý nhân viên**, bấm **“Thêm nhân viên mới”**.
2. Nhập đầy đủ các thông tin yêu cầu, ví dụ:
   - Họ tên
   - Ngày sinh
   - Vị trí
   - **Mức lương** (lương cơ bản theo tháng – dùng để tính lương ở tab Lương)
   - Ngày vào
   - (Tùy cấu hình) Ngày kết thúc, ảnh đại diện, …
3. Bấm **Lưu**.
4. Nhân viên mới sẽ xuất hiện trong danh sách.

### 4.4. Sửa thông tin nhân viên (chỉ Admin)

1. Tại danh sách nhân viên, chọn dòng nhân viên cần sửa, bấm **“Sửa”**.
2. Thay đổi các thông tin mong muốn (ví dụ: chức danh, mức lương…).
3. Bấm **Lưu** để cập nhật.

### 4.5. Xóa nhân viên (chỉ Admin)

1. Tại danh sách nhân viên, chọn dòng nhân viên cần xóa, bấm **“Xóa”**.
2. Xác nhận xóa khi hệ thống hỏi lại.
3. Nhân viên sẽ được gỡ khỏi danh sách (dữ liệu liên quan có thể vẫn được lưu trong hệ thống tùy cấu hình).

---

## 5. CHẤM CÔNG (ATTENDANCE)

> Lưu ý: Phần này mô tả tổng quan, màn hình thực tế có thể được bổ sung/điều chỉnh tùy phiên bản.

Ứng dụng có mô hình dữ liệu **Attendance** (Chấm công) ghi lại:
- Ngày làm việc
- Nhân viên (Employee)
- Có mặt hay không (**IsPresent**)
- Số ngày công (**WorkDay**)
- Thời gian check-in / check-out

Từ các bản ghi chấm công này, hệ thống dùng để **tự động tính lương tháng** ở tab **Lương**.

---

## 6. TỰ ĐỘNG TÍNH LƯƠNG THÁNG (TAB “LƯƠNG”)

Truy cập: trên menu, chọn **“Lương”**.

### 6.1. Mục đích

Tab **Lương** cho phép:
- Xem **bảng lương tháng** của toàn bộ nhân viên đang làm việc trong tháng được chọn.
- Tự động tính **lương thực lĩnh** dựa trên:
  - **Mức lương cơ bản theo tháng** của từng nhân viên (đã nhập tại QL Nhân viên).
  - **Số ngày công** có mặt trong tháng (lấy từ dữ liệu chấm công).

### 6.2. Chọn tháng / năm

Trên đầu trang Lương có:
- Ô chọn **Tháng** (1–12).
- Ô chọn **Năm** (mặc định là năm hiện tại, có thể chọn các năm lân cận).
- Nút **“Xem bảng lương”**.

Các bước:
1. Chọn **Tháng**.
2. Chọn **Năm**.
3. Bấm **“Xem bảng lương”**.
4. Hệ thống sẽ tải dữ liệu lương của tháng/năm vừa chọn.

### 6.3. Cách hệ thống tự động tính lương

Đối với mỗi nhân viên:
- Hệ thống xác định **nhân viên đang làm việc trong tháng**:
  - Ngày vào làm **≤ ngày cuối của tháng**.
  - Ngày kết thúc (nếu có) **≥ ngày đầu của tháng**.
- Hệ thống thống kê **số ngày công (WorkedDays)** trong tháng từ dữ liệu chấm công.
- **Số ngày chuẩn trong tháng** hiện được cấu hình cố định là **22 ngày**.

**Công thức tính lương tháng:**

- Nếu **có chấm công** trong tháng:
  - \\( Lương\ thực\ lĩnh = Lương\ cơ\ bản × \\frac{Số\ ngày\ công}{22} \\)
- Nếu **không có bản ghi chấm công** trong tháng:
  - Hệ thống hiểu là đủ công → tính **đủ 22 ngày** (nhận **đủ lương cơ bản**).

> Ghi chú: Nếu số ngày công vượt quá 22 (do dữ liệu), hệ thống tự động giới hạn lại tối đa 22 ngày.

### 6.4. Bảng lương hiển thị

Sau khi chọn tháng/năm và bấm **“Xem bảng lương”**, bảng sẽ hiển thị:
- **STT**
- **Mã NV**
- **Họ tên**
- **Vị trí**
- **Lương cơ bản (tháng)** – giá trị nhập ở QL Nhân viên
- **Số ngày công / 22** – tỷ lệ số ngày làm việc trên số ngày chuẩn
- **Lương thực lĩnh** – kết quả tính toán theo công thức trên

Ở phía dưới bảng có:
- **Tổng quỹ lương tháng**: tổng tất cả **lương thực lĩnh** của các nhân viên trong tháng đó.

Nếu trong tháng không có nhân viên nào (hoặc không ai thỏa điều kiện đang làm việc), hệ thống sẽ hiển thị thông báo tương ứng.

---

## 7. GỢI Ý QUY TRÌNH LÀM VIỆC

1. **Admin** tạo tài khoản và phân quyền cho người dùng phù hợp.
2. **Admin** vào **QL Nhân viên**:
   - Thêm đầy đủ thông tin nhân viên, đặc biệt là **mức lương cơ bản** và **ngày vào làm**.
3. Người phụ trách chấm công đảm bảo dữ liệu **Attendance** được cập nhật đầy đủ mỗi ngày.
4. Cuối tháng (hoặc bất kỳ lúc nào cần xem), vào tab **Lương**:
   - Chọn **Tháng/Năm**.
   - Xem **bảng lương** và **tổng quỹ lương**.
5. Dữ liệu từ tab Lương có thể dùng để:
   - Lập bảng lương chi tiết.
   - Đối chiếu với bộ phận kế toán.

---

## 8. CÂU HỎI THƯỜNG GẶP

**Hỏi:** Không xem được tab “Lương”?  
**Đáp:** Hãy chắc chắn bạn đã **đăng nhập**. Ứng dụng yêu cầu đăng nhập để xem các chức năng quản lý.

**Hỏi:** Vì sao lương tháng của nhân viên bằng 0?  
**Đáp:** Kiểm tra:
- Mức lương cơ bản của nhân viên ở phần **QL Nhân viên** có bằng 0 không.
- Dữ liệu chấm công có sai / thiếu không (ví dụ không đánh dấu có mặt, `IsPresent` = false).

**Hỏi:** Muốn thay đổi cách tính (ví dụ: số ngày chuẩn khác 22, cộng thêm OT, phụ cấp…)?  
**Đáp:** Cần bên kỹ thuật điều chỉnh lại logic tính trong hệ thống. Bạn có thể mô tả yêu cầu chi tiết cho lập trình viên để cập nhật.

---

## 9. LIÊN HỆ / HỖ TRỢ

Khi gặp lỗi hoặc cần nâng cấp tính năng:
- Ghi lại **thời điểm xảy ra**, thao tác đã thực hiện, và **thông báo lỗi (nếu có)**.
- Gửi cho **quản trị hệ thống / lập trình viên** để được hỗ trợ nhanh nhất.

Ứng dụng MiniERP được thiết kế đơn giản, dễ dùng. Chỉ cần làm quen vài lần, bạn có thể quản lý nhân viên, chấm công và xem bảng lương hàng tháng một cách nhanh chóng và chính xác.

