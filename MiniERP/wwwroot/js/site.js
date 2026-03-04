// ---------------------------------------------------------
// NAMHA TECHNOLOGY - Global Scripts (site.js)
// ---------------------------------------------------------

document.addEventListener("DOMContentLoaded", function () {
    // 1. Khởi tạo Đồng hồ thời gian thực
    initDigitalClock();

    // 2. Khởi tạo Tin tức (Chỉ chạy nếu có thẻ chứa tin trên trang)
    if (document.getElementById('news-feed')) {
        initNewsFeed();
    }

    // 3. Khởi tạo Search Box logic (Xử lý các sự kiện bổ trợ)
    initSearchBox();
});

/**
 * Hàm cập nhật đồng hồ và ngày tháng
 */
function initDigitalClock() {
    const clockElement = document.getElementById('digital-clock');
    const dateElement = document.getElementById('current-date');

    if (!clockElement || !dateElement) return;

    function update() {
        const now = new Date();
        clockElement.textContent = now.toLocaleTimeString('vi-VN');
        dateElement.textContent = now.toLocaleDateString('vi-VN', { 
            weekday: 'long', 
            year: 'numeric', 
            month: 'long', 
            day: 'numeric' 
        });
    }

    setInterval(update, 1000);
    update();
}

/**
 * Hàm lấy tin tức từ RSS VnExpress (qua Proxy rss2json)
 */
function initNewsFeed() {
    const newsFeed = document.getElementById('news-feed');
    const loader = document.getElementById('news-loader');
    if (!newsFeed) return;

    const rssUrl = 'https://vnexpress.net/rss/tin-moi-nhat.rss';
    const apiUrl = `https://api.rss2json.com/v1/api.json?rss_url=${encodeURIComponent(rssUrl)}`;

    fetch(apiUrl)
        .then(res => res.json())
        .then(data => {
            if (loader) loader.remove();
            
            if (data.status === 'ok') {
                data.items.slice(0, 6).forEach(item => {
                    const html = `
                        <div class="news-item py-3" onclick="window.open('${item.link}', '_blank')" style="cursor:pointer; border-bottom: 1px solid #eee;">
                            <h6 class="mb-1 fw-bold small text-dark">${item.title}</h6>
                            <div class="d-flex justify-content-between align-items-center">
                                <span class="text-muted" style="font-size: 0.7rem;">${new Date(item.pubDate).toLocaleTimeString('vi-VN')}</span>
                                <i class="bi bi-chevron-right text-muted small"></i>
                            </div>
                        </div>
                    `;
                    newsFeed.insertAdjacentHTML('beforeend', html);
                });
            } else {
                newsFeed.innerHTML = '<p class="text-muted text-center py-5">Không thể tải tin tức lúc này.</p>';
            }
        })
        .catch(err => {
            if (loader) loader.remove();
            newsFeed.innerHTML = '<p class="text-muted text-center py-5">Lỗi kết nối nguồn tin.</p>';
        });
}

/**
 * HÀM SEARCH CHÍNH XÁC: Xử lý tìm kiếm qua Fetch và hiển thị Modal kết quả
 */
function searchEmployee() {
    var keyword = document.getElementById("searchBox").value;
    fetch('/Employee/Search?keyword=' + encodeURIComponent(keyword))
        .then(response => response.text())
        .then(html => {
            document.getElementById("searchResults").innerHTML = html;
            var modalElement = document.getElementById("searchModal");
            if (modalElement) {
                var modal = new bootstrap.Modal(modalElement);
                modal.show();
            }
        })
        .catch(err => console.error("Search error:", err));
}

/**
 * Hàm xử lý logic bổ trợ cho ô tìm kiếm (Search Box)
 */
function initSearchBox() {
    const searchInput = document.getElementById('searchBox');
    if (!searchInput) return;

    // Hiệu ứng focus: Tự động bôi đen văn bản khi click vào nếu đã có chữ
    searchInput.addEventListener('focus', function() {
        if (this.value.length > 0) {
            this.select();
        }
    });

    // Xử lý phím tắt: Nhấn '/' để focus vào ô tìm kiếm nhanh
    document.addEventListener('keydown', function(e) {
        if (e.key === '/' && !['INPUT', 'TEXTAREA'].includes(document.activeElement.tagName)) {
            e.preventDefault();
            searchInput.focus();
        }
    });

    // Cho phép nhấn Enter để gọi hàm searchEmployee
    searchInput.addEventListener('keypress', function(e) {
        if (e.key === 'Enter') {
            e.preventDefault();
            searchEmployee();
        }
    });
}

/**
 * Hàm kiểm tra đăng nhập
 */
function isLoggedIn() {
    return localStorage.getItem("userToken") !== null;
}

function goToEmployeePage() {
    if (!isLoggedIn()) {
        var modalElement = document.getElementById("loginRequiredModal");
        if (modalElement) {
            var modal = new bootstrap.Modal(modalElement);
            modal.show();
        }
        return;
    }
    window.location.href = "/Employee/EmployeeManagement";
}
