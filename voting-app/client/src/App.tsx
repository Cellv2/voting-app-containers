import React from "react";
import { submitVoteAsync } from "./api/vote";
import "./App.css";

function App() {
    return (
        <div className="App">
            <header className="App-header">
                <button
                    className="big-button"
                    onClick={submitVoteAsync}
                    value="1"
                >
                    Option 1
                </button>
                <br />
                <button
                    className="big-button"
                    onClick={submitVoteAsync}
                    value="2"
                >
                    Option 2
                </button>
            </header>
        </div>
    );
}

export default App;
