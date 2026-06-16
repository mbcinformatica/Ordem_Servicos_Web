// Funcao para iniciar o progresso da barra, fazendo requisições periódicas para obter o progresso atual
function iniciarProgresso(url, barraId, intervaloMs = 1000) {
    let intervalo = setInterval(() => {
        fetch(url)
            .then(response => response.json())
            .then(data => {
                let barra = document.getElementById(barraId);
                if (!barra) return;

                barra.style.width = data.progresso + "%";
                barra.setAttribute("aria-valuenow", data.progresso);
                barra.innerText = data.progresso + "%";

                if (data.progresso >= 100) {
                    clearInterval(intervalo);
                }
            })
            .catch(err => {
                console.error("Erro ao atualizar progresso:", err);
                clearInterval(intervalo);
            });
    }, intervaloMs);
}