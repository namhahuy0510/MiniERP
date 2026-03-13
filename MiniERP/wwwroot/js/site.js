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

    // Lấy username hiện tại từ layout (server sẽ render vào data-attr trên body nếu cần)
    const currentUser = document.body.getAttribute("data-current-user") || "";

    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/chatHub")
        .build();

    connection.on("ReceiveMessage", (fromUser, toUser, message, sentAt) => {
        const messagesList = document.getElementById("messagesList");
        if (!messagesList) return;

        // Chỉ hiển thị tin nhắn nếu mình là người gửi hoặc người nhận
        if (!currentUser || (currentUser !== fromUser && currentUser !== toUser)) {
            return;
        }

        const li = document.createElement("li");
        const isSelf = currentUser === fromUser;

        const mainText = isSelf
            ? `Bạn → ${toUser}: ${message}`
            : `${fromUser} → Bạn: ${message}`;

        li.innerHTML = `
            <div>${mainText}</div>
            <div class="chat-meta">${sentAt || ""}</div>
        `;

        li.className = "chat-bubble " + (isSelf ? "chat-bubble-self" : "chat-bubble-other");

        messagesList.appendChild(li);
        messagesList.scrollTop = messagesList.scrollHeight;
    });

    connection.start().catch(err => console.error(err.toString()));

    document.getElementById("sendButton").addEventListener("click", () => {
        const recipientInput = document.getElementById("recipientInput");
        const messageInput = document.getElementById("messageInput");
        if (!recipientInput || !messageInput) return;

        const fromUser = currentUser;
        const toUser = recipientInput.value.trim();
        const message = messageInput.value.trim();

        if (fromUser && toUser && message) {
            connection.invoke("SendMessage", fromUser, toUser, message)
                .catch(err => console.error(err.toString()));
            messageInput.value = "";
        }
    });

    const toggleBtn = document.getElementById("toggleChat");
    if (toggleBtn) {
        toggleBtn.addEventListener("click", () => {
            chatBox.classList.toggle("collapsed");
        });
    }

    const launcher = document.getElementById("chatLauncher");
    if (launcher) {
        launcher.addEventListener("click", () => {
            const isOpen = chatBox.classList.contains("chat-open");
            chatBox.classList.toggle("chat-open", !isOpen);
            chatBox.classList.toggle("chat-closed", isOpen);
        });
    }
}

// Khởi tạo tất cả
document.addEventListener("DOMContentLoaded", function () {
    initDigitalClock();
    if (document.getElementById('news-feed')) initNewsFeed();
    initSearchBox();
    initChatBox();
    initInboxPage();
});

// Hộp thư đến: đánh dấu đã đọc / xóa, lưu phía client (localStorage)
function initInboxPage() {
    const inboxList = document.getElementById("inboxList");
    if (!inboxList) return;

    const items = Array.from(inboxList.querySelectorAll(".inbox-item"));
    let unreadCount = 0;

    items.forEach(item => {
        const id = item.getAttribute("data-id");
        if (!id) return;

        const deleted = localStorage.getItem(`inbox_deleted_${id}`) === "1";
        if (deleted) {
            item.remove();
            return;
        }

        const isRead = localStorage.getItem(`inbox_read_${id}`) === "1";
        const dot = item.querySelector(".notification-dot");

        if (isRead) {
            item.classList.add("notification-read");
            item.classList.remove("notification-unread");
            if (dot) dot.classList.add("d-none");
        } else {
            item.classList.add("notification-unread");
            item.classList.remove("notification-read");
            if (dot) dot.classList.remove("d-none");
            unreadCount++;
        }
    });

    // Cập nhật badge đếm và chấm đỏ tiêu đề
    const inboxCounter = document.getElementById("inboxCounter");
    if (inboxCounter) {
        const total = inboxList.querySelectorAll(".inbox-item").length;
        inboxCounter.textContent = `${total} thông báo (chưa đọc: ${unreadCount})`;
    }
    const inboxUnreadDot = document.getElementById("inboxUnreadDot");
    if (inboxUnreadDot) {
        if (unreadCount > 0) inboxUnreadDot.classList.remove("d-none");
        else inboxUnreadDot.classList.add("d-none");
    }

    // Gắn sự kiện nút
    inboxList.addEventListener("click", function (e) {
        const markBtn = e.target.closest(".btn-mark-read");
        const deleteBtn = e.target.closest(".btn-delete-noti");

        if (markBtn) {
            const id = markBtn.getAttribute("data-id");
            const item = inboxList.querySelector(`.inbox-item[data-id="${id}"]`);
            if (!id || !item) return;

            const isRead = item.classList.contains("notification-read");
            if (isRead) {
                // Bỏ đánh dấu đã đọc
                item.classList.remove("notification-read");
                item.classList.add("notification-unread");
                localStorage.removeItem(`inbox_read_${id}`);
                const dot = item.querySelector(".notification-dot");
                if (dot) dot.classList.remove("d-none");
            } else {
                // Đánh dấu đã đọc
                item.classList.add("notification-read");
                item.classList.remove("notification-unread");
                localStorage.setItem(`inbox_read_${id}`, "1");
                const dot = item.querySelector(".notification-dot");
                if (dot) dot.classList.add("d-none");
            }

            // Recalc
            initInboxPage();
        }

        if (deleteBtn) {
            const id = deleteBtn.getAttribute("data-id");
            const item = inboxList.querySelector(`.inbox-item[data-id="${id}"]`);
            if (!id || !item) return;

            localStorage.setItem(`inbox_deleted_${id}`, "1");
            item.remove();

            // Recalc
            initInboxPage();
        }
    });
}
