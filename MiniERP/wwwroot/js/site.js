function searchEmployee() {
    var keyword = document.getElementById("searchBox").value;
    fetch('/Employee/Search?keyword=' + encodeURIComponent(keyword))
        .then(response => response.text())
        .then(html => {
            document.getElementById("searchResults").innerHTML = html;
            var modal = new bootstrap.Modal(document.getElementById("searchModal"));
            modal.show();
        });
}
