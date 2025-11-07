export class BaseParams {
    pageNumber: number;
    pageSize: number;
    sortBy: string;
    searchBy: string;

    constructor(pageSize: number = 10, pageNumber: number = 1, sortBy: string = '', searchBy: string = '') {
        this.pageSize = pageSize;
        this.pageNumber = pageNumber;
        this.sortBy = sortBy;
        this.searchBy = searchBy;
    }
}