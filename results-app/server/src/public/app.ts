const setupNodes = () => {
    const root = document.getElementById("root");
    if (!root) return;

    const testNode = document.createElement("p");
    testNode.innerText = "testing the app.js";

    root.appendChild(testNode);
};

setupNodes();
