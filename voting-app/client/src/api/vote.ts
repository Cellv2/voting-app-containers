export const submitVoteAsync = async (
    vote: React.MouseEvent<HTMLButtonElement, MouseEvent>
) => {
    vote.persist();
    const { value } = vote.currentTarget;

    // TODO: update the URL to be dynamic
    // const url = `http://localhost:8080/vote/${value}`;
    const url = `http://localhost:5050/vote/${value}`;

    await fetch(url, {
        method: "POST",
    });
};
