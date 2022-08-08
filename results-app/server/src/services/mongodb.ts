import mongoose from "mongoose";
import { MONGODB_CONNSTRING } from "../constants/mongodb";
import { Vote } from "../models/Vote";

interface MongoDbServiceConstructor {
    new (connectionUri: string): MongoDbServiceInterface;
}

interface MongoDbServiceInterface {
    findAllVotes: () => Promise<Vote[]>;
}

const mongoDbService: MongoDbServiceConstructor = class MonboDbService
    implements MongoDbServiceInterface
{
    constructor(connectionUri: string) {
        console.log(`connected with ${connectionUri}`);
        mongoose.connect(connectionUri);
    }

    findAllVotes = async (): Promise<Vote[]> => {
        const findAllVotes = await Vote.find({});
        console.log(findAllVotes);
        return findAllVotes;
    };
};

export const mongoDbServiceSingleton = new mongoDbService(MONGODB_CONNSTRING);

export default mongoDbServiceSingleton;
