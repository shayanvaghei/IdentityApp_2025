export class PaginatedResult<T> {
    items: T[];
    totalItemsCount: number;
    pageNumber: number;
    pageSize: number;
    totalPages: number;

    constructor(items: T[], totalItemsCount: number, pageNumber: number, pageSize: number, totalPages: number) {
        this.totalItemsCount = totalItemsCount;
        this.pageNumber = pageNumber;
        this.pageSize = pageSize;
        this.totalPages = totalPages;
        this.items = items;
    }
}