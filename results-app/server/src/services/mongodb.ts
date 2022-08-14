import mongoose from "mongoose";
import { MONGODB_CONNSTRING } from "../constants/mongodb";
import { Vote, VoteModel } from "../models/Vote";

interface MongoDbServiceConstructor {
    new (connectionUri: string): MongoDbServiceInterface;
}

interface MongoDbServiceInterface {
    findAllVotes: () => Promise<Vote[]>;
}

// * models are buffered into mongoose and are not initialised in this service
// https://mongoosejs.com/docs/connections.html#buffering
const mongoDbService: MongoDbServiceConstructor = class MonboDbService
    implements MongoDbServiceInterface
{
    #connection: Promise<typeof mongoose> | null;
    private _connectionUri: string;

    constructor(connectionUri: string) {
        this.#connection = null;
        this._connectionUri = connectionUri;
    }

    // while mongoose should be able to queue requests, it's better safe than sorry
    // https://mongoosejs.com/docs/connections.html#connections
    private ensureConnected = async () => {
        if (!this.#connection) {
            this.#connection = mongoose.connect(this._connectionUri);
        }

        return this.#connection;
    };

    findAllVotes = async (): Promise<Vote[]> => {
        await this.ensureConnected();

        // TODO: is there a way to make this more type safe?
        const findAllVotes = VoteModel.find({});

        return findAllVotes;
    };
};

export const mongoDbServiceSingleton = new mongoDbService(MONGODB_CONNSTRING);

export default mongoDbServiceSingleton;
