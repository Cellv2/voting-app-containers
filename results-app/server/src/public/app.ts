import { Vote } from "../models/Vote";

let voteOneContainer: HTMLParagraphElement;
let voteTwoContainer: HTMLParagraphElement;

const setupVoteElements = (): void => {
    const root = document.getElementById("root");
    if (!root) return;

    voteOneContainer = document.createElement("p");
    voteOneContainer.id = "voteOneContainer";

    voteTwoContainer = document.createElement("p");
    voteTwoContainer.id = "voteTwoContainer";

    root.append(voteOneContainer, voteTwoContainer);
};

const updateVoteCounts = (votes: Vote[]): void => {
    votes.forEach((voteItem) => {
        const { count, voteOption } = voteItem;
        if (voteOption === "1") {
            voteOneContainer.innerText = `Votes for 1: ${count ?? 0}`;
        } else if (voteOption === "2") {
            voteTwoContainer.innerText = `Votes for 2: ${count ?? 0}`;
        }
    });
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
            const messageBody = JSON.parse(webSocketMessage.data) as Vote[];
            updateVoteCounts(messageBody);
        };
    } catch (err) {
        console.error(err);
    }
};

(async () => {
    setupVoteElements();
    await setupClientWs();
})();
