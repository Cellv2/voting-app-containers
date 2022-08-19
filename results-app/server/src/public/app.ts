const addMessage = (message: string) => {
    const root = document.getElementById("root");
    if (!root) return;

    const testNode = document.createElement("p");
    testNode.innerText = message;

    root.appendChild(testNode);
};

const connectToServer = async (): Promise<WebSocket> => {
    try {
        const ws = new WebSocket("ws://localhost:8085/ws");
        return new Promise((resolve, reject) => {
            const timer = setInterval(() => {
                if (ws.readyState === 1) {
                    clearInterval(timer);
                    resolve(ws);
                }
            }, 10);
        });
    } catch (err) {
        console.log(`wss - client: an error occurred: \n${err}`);
        return Promise.reject(err);
    }
};

const setupClientWs = async () => {
    try {
        const ws = await connectToServer();

        ws.onmessage = (webSocketMessage: { data: string }) => {
            const messageBody = JSON.parse(webSocketMessage.data);
            addMessage(messageBody);
        };
    } catch (err) {
        console.error(err);
    }
};

(async () => {
    await setupClientWs();
})();
