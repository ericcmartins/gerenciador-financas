const CONFIG = {
    API: {
        BASE_URL: 'http://localhost:5036'
    },

    APP: {
        NOME: 'FinOn',
        VERSAO: '1.0.0',
        ITENS_POR_PAGINA: 10,
        LOCALIDADE: 'pt-BR',
        MOEDA: 'BRL'
    },
    
    UTIL: {
        /**
         * Formata um número para o padrão de moeda brasileiro.
         */
        formatarMoeda: (valor) => {
            return new Intl.NumberFormat(CONFIG.APP.LOCALIDADE, {
                style: 'currency',
                currency: CONFIG.APP.MOEDA
            }).format(valor || 0);
        },

        /**
         * Formata uma data (string ISO com ou sem hora) para o padrão brasileiro (dd/mm/aaaa).
         * @param {string | Date} data A data a ser formatada.
         * @returns {string} A data formatada.
         */
        formatarData: (data) => {
            if (!data) return '';
            // Cria o objeto de data diretamente da string ISO.
            // Adiciona a opção timeZone: 'UTC' para evitar que a data mude por causa do fuso horário do navegador.
            const dataObj = new Date(data);
            return dataObj.toLocaleDateString(CONFIG.APP.LOCALIDADE, { timeZone: 'UTC' });
        },
        
        /**
         * Formata uma data (string ISO com ou sem hora) para ser usada em um campo <input type="date"> (aaaa-mm-dd).
         * @param {string | Date} data A data a ser formatada.
         * @returns {string} A data no formato 'YYYY-MM-DD'.
         */
        formatarDataParaInput: (data) => {
            if (!data) return '';
            const dataObj = new Date(data);
            // Usa métodos UTC para pegar o ano, mês e dia corretos, ignorando o fuso horário.
            const ano = dataObj.getUTCFullYear();
            const mes = String(dataObj.getUTCMonth() + 1).padStart(2, '0');
            const dia = String(dataObj.getUTCDate()).padStart(2, '0');
            return `${ano}-${mes}-${dia}`;
        },

        /**
         * Gera uma lista de cores elegantes para usar em gráficos.
         */
        gerarCores: (quantidade) => {
            const coresBase = [
                '#005a44', '#10b981', '#4ade80', '#374151', 
                '#6b7280', '#a7f3d0', '#00755b', '#9ca3af'
            ];
            const coresGeradas = [];
            for (let i = 0; i < quantidade; i++) {
                coresGeradas.push(coresBase[i % coresBase.length]);
            }
            return coresGeradas;
        },
        
        /**
         * Função "Debounce" para evitar execuções repetidas.
         */
        debounce: (func, espera) => {
            let timeout;
            return function executedFunction(...args) {
                const later = () => {
                    clearTimeout(timeout);
                    func(...args);
                };
                clearTimeout(timeout);
                timeout = setTimeout(later, espera);
            };
        },
        
        /**
         * Exibe um spinner de carregamento em um botão.
         */
        mostrarCarregamento: (botao) => {
            if (botao) {
                const textoBotao = botao.querySelector('.botao-texto');
                const carregandoDiv = botao.querySelector('.botao-carregando');
                
                if(textoBotao) textoBotao.style.visibility = 'hidden';
                if(carregandoDiv) carregandoDiv.style.display = 'flex';
                botao.disabled = true;
            }
        },
        
        /**
         * Esconde o spinner de carregamento de um botão.
         */
        esconderCarregamento: (botao) => {
            if (botao) {
                const textoBotao = botao.querySelector('.botao-texto');
                const carregandoDiv = botao.querySelector('.botao-carregando');

                if(textoBotao) textoBotao.style.visibility = 'visible';
                if(carregandoDiv) carregandoDiv.style.display = 'none';
                botao.disabled = false;
            }
        },

        /**
         * Valida se um email tem um formato válido.
         */
        validarEmail: (email) => {
            const re = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
            return re.test(String(email).toLowerCase());
        },
    }
};

export { CONFIG };
