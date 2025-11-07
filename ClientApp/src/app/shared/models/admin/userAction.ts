export class UserAction {
    userId: number;
    action: string;
    daysToLock: number | undefined;

    constructor(userId: number, action: string, daysToLock: number | undefined = undefined) {
        this.userId = userId;
        this.action = action;
        this.daysToLock = daysToLock;
    }
}