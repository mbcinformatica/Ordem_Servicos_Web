function initSortableTable({
    tableSelector = "table",
    searchBoxSelector = "#searchBox",
    targetTableBody = "#clientesTable",
    totalRegSelector = "#totalRegistros",
    colMap = {},
    initialCol = 0,
    initialDirection = true,
    searchUrl = "",
    indexUrl = "",
    defaultSearchColumn = "",
    placeholders = {}
}) {
    let currentSortedCol = initialCol;
    let currentDirection = initialDirection;

    const table = document.querySelector(tableSelector);
    const searchBox = document.querySelector(searchBoxSelector);

    if (!table) return;

    // Eventos de clique nos cabeçalhos
    table.querySelectorAll("th.sortable").forEach((th) => {
        th.style.cursor = "pointer";
        th.addEventListener("click", function () {
            const colIndex = this.cellIndex;
            currentSortedCol = colIndex;
            currentDirection = !currentDirection;
            sortTable(table, colIndex, this, currentDirection);

            // Atualiza placeholder conforme coluna
            if (searchBox) {
                let colName = colMap[currentSortedCol] || defaultSearchColumn;
                if (placeholders[colName]) {
                    searchBox.placeholder = placeholders[colName];
                }
                searchBox.focus();
            }
        });
    });

    // Ordena inicialmente pela coluna definida
    const thInicial = table.querySelectorAll("th")[currentSortedCol];
    if (thInicial) {
        sortTable(table, currentSortedCol, thInicial, currentDirection);
    }

    // Atualiza placeholder inicial e foca no campo
    if (searchBox) {
        let colName = colMap[currentSortedCol] || defaultSearchColumn;
        if (placeholders[colName]) {
            searchBox.placeholder = placeholders[colName];
        }
        searchBox.focus();
    }

    // Foca no campo de pesquisa ao clicar em qualquer parte da tabela
    const tbody = table.querySelector("tbody");
    if (tbody && searchBox) {
        tbody.addEventListener("click", function () {
            searchBox.focus();
        });
    }

    // Busca dinâmica
    if (searchBox) {
        searchBox.addEventListener("keyup", function () {
            var searchValue = this.value.trim();

            if (searchValue.length > 0) {
                let colName = colMap[currentSortedCol] || defaultSearchColumn;

                fetch(searchUrl + "?search=" + encodeURIComponent(searchValue) + "&column=" + encodeURIComponent(colName))
                    .then(response => response.text())
                    .then(html => {
                        document.querySelector(targetTableBody).innerHTML = html;

                        var total = document.querySelectorAll(targetTableBody + " tr").length;
                        document.querySelector(totalRegSelector).innerText = "Total de Registros: " + total;

                        if (typeof aplicarFormatacaoTabela === "function") {
                            aplicarFormatacaoTabela();
                        }

                        // Reaplica ordenação na coluna atual
                        const thAtual = table.querySelectorAll("th")[currentSortedCol];
                        if (thAtual) {
                            sortTable(table, currentSortedCol, thAtual, currentDirection);
                        }
                    });
            } else {
                window.location.href = indexUrl;
            }
        });
    }

    function sortTable(table, colIndex, th, direction) {
        const tbody = table.querySelector("tbody");
        if (!tbody) return;

        const rows = Array.from(tbody.querySelectorAll("tr"));

        rows.sort((a, b) => {
            let aCell = a.children[colIndex];
            let bCell = b.children[colIndex];
            if (!aCell || !bCell) return 0;

            let aText = aCell.textContent.trim();
            let bText = bCell.textContent.trim();

            if (colIndex === 1) { // coluna ID
                let aVal = parseInt(aText, 10) || 0;
                let bVal = parseInt(bText, 10) || 0;
                return direction ? aVal - bVal : bVal - aVal;
            }

            const cmp = aText.localeCompare(bText, undefined, { sensitivity: 'base', numeric: true });
            return direction ? cmp : -cmp;
        });

        const fragment = document.createDocumentFragment();
        rows.forEach(row => fragment.appendChild(row));
        tbody.appendChild(fragment);

        // Remove ícones e destaque anteriores
        table.querySelectorAll("th.sortable").forEach(header => {
            header.textContent = header.textContent.replace(/[\u2191\u2193]/g, "").trim();
            header.classList.remove("active");
        });

        if (th) {
            th.textContent = th.textContent + (direction ? " ↑" : " ↓");
            th.classList.add("active");
        }
    }
}