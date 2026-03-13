// Đồng hồ
function initDigitalClock() {
    const clockElement = document.getElementById('digital-clock');
    const dateElement = document.getElementById('current-date');
    if (!clockElement || !dateElement) return;

    function update() {
        const now = new Date();
        clockElement.textContent = now.toLocaleTimeString('vi-VN');
        dateElement.textContent = now.toLocaleDateString('vi-VN', { 
            weekday: 'long', year: 'numeric', month: 'long', day: 'numeric' 
        });
    }
    setInterval(update, 1000);
    update();
}

// Tin tức
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
                        </div>`;
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

// Tìm kiếm nhân viên
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

// Search Box
function initSearchBox() {
    const searchInput = document.getElementById('searchBox');
    if (!searchInput) return;

    searchInput.addEventListener('focus', function() {
        if (this.value.length > 0) this.select();
    });

    document.addEventListener('keydown', function(e) {
        if (e.key === '/' && !['INPUT', 'TEXTAREA'].includes(document.activeElement.tagName)) {
            e.preventDefault();
            searchInput.focus();
        }
    });

    searchInput.addEventListener('keypress', function(e) {
        if (e.key === 'Enter') {
            e.preventDefault();
            searchEmployee();
        }
    });
}

// Đăng nhập
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

// ChatBox
function initChatBox() {
    const chatBox = document.getElementById("chatBox");
    if (!chatBox) return;

    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/chatHub")
        .build();

    connection.on("ReceiveMessage", (user, message) => {
        const msg = `${user}: ${message}`;
        const li = document.createElement("li");
        li.textContent = msg;
        li.className = "list-group-item";
        document.getElementById("messagesList").appendChild(li);
    });

    connection.start().catch(err => console.error(err.toString()));

    document.getElementById("sendButton").addEventListener("click", () => {
        const user = document.getElementById("userInput").value;
        const message = document.getElementById("messageInput").value;
        if (user && message) {
            connection.invoke("SendMessage", user, message).catch(err => console.error(err.toString()));
            document.getElementById("messageInput").value = "";
        }
    });

    const toggleBtn = document.getElementById("toggleChat");
    if (toggleBtn) {
        toggleBtn.addEventListener("click", () => {
            chatBox.classList.toggle("collapsed");
        });
    }
}

// Khởi tạo tất cả
document.addEventListener("DOMContentLoaded", function () {
    initDigitalClock();
    if (document.getElementById('news-feed')) initNewsFeed();
    initSearchBox();
    initChatBox();
});
