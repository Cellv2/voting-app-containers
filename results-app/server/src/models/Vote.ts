import { model, Schema } from "mongoose";

export interface Vote {
    voteOption: string;
    count: number;
}

const voteSchema = new Schema<Vote>({
    voteOption: { type: String, required: true },
    count: { type: Number, required: true },
});

// https://github.com/Automattic/mongoose/blob/master/examples/schema/schema.js

// https://mongoosejs.com/docs/connections.html#buffering
export const VoteModel = model<Vote>("vote", voteSchema);
