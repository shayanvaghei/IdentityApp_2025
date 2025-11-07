import { BaseParams } from "./baseParams";

export class AllUsersParams extends BaseParams {
    csvRoles: string | undefined;
    lockUnlock: string | undefined;
    activation: string | undefined;

    constructor(pageSize: number = 10, pageNumber: number = 1, sortBy: string = '', searchBy: string = '',
        csvRoles: string | undefined = undefined,
        lockUnlock: string | undefined = undefined,
        activation: string | undefined = undefined) {
        super(pageSize, pageNumber, sortBy, searchBy);

        this.csvRoles = csvRoles;
        this.lockUnlock = lockUnlock;
        this.activation = activation;
    }
}