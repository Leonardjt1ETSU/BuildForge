document.addEventListener("DOMContentLoaded", () => {
    const searchInput = document.getElementById("catalogSearch");
    const tableRows = document.querySelectorAll("#componentTable tbody tr");

    if (!searchInput || tableRows.length === 0) {
        return;
    }

    searchInput.addEventListener("input", () => {
        const searchText = searchInput.value.toLowerCase();

        tableRows.forEach(row => {
            const rowText = row.innerText.toLowerCase();

            if (rowText.includes(searchText)) {
                row.style.display = "";
            } else {
                row.style.display = "none";
            }
        });
    });
});