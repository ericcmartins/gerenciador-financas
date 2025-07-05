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
         * Ex: formatarMoeda(1234.5) => "R$ 1.234,50"
         * @param {number} valor 
         * @returns {string} 
         */
        formatarMoeda: (valor) => {
            return new Intl.NumberFormat(CONFIG.APP.LOCALIDADE, {
                style: 'currency',
                currency: CONFIG.APP.MOEDA
            }).format(valor || 0);
        },

        /**
         * Formata uma data para o padrão brasileiro (dd/mm/aaaa).
         * @param {string | Date} data A data a ser formatada.
         * @returns {string} A data formatada.
         */
        formatarData: (data) => {
            if (!data) return '';
            // Adiciona 'T00:00:00' para garantir que a data seja interpretada em fuso local
            const dataObj = new Date(data + 'T00:00:00');
            return dataObj.toLocaleDateString(CONFIG.APP.LOCALIDADE);
        },
        
        /**
         * Formata uma data para ser usada em um campo <input type="date"> (aaaa-mm-dd).
         * @param {string | Date} data A data a ser formatada.
         * @returns {string} A data no formato 'YYYY-MM-DD'.
         */
        formatarDataParaInput: (data) => {
            if (!data) return '';
            const dataObj = new Date(data + 'T00:00:00');
            const ano = dataObj.getFullYear();
            const mes = String(dataObj.getMonth() + 1).padStart(2, '0');
            const dia = String(dataObj.getDate()).padStart(2, '0');
            return `${ano}-${mes}-${dia}`;
        },
        
        /**
         * Função "Debounce" para evitar execuções repetidas de uma função.
         * Útil em campos de busca, para não fazer uma chamada à API a cada tecla digitada.
         * @param {Function} func A função a ser executada.
         * @param {number} espera O tempo de espera em milissegundos.
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
         * Exibe um spinner de carregamento em um elemento do botão.
         * @param {HTMLElement} botao O elemento do botão.
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
         * Esconde o spinner de carregamento e restaura o texto de um botão.
         * @param {HTMLElement} botao O elemento do botão.
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
         * @param {string} email O email a ser validado.
         * @returns {boolean}
         */
        validarEmail: (email) => {
            const re = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
            return re.test(String(email).toLowerCase());
        },
    }
};

export { CONFIG };





