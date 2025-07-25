export class ApiResponse<T> {
    statusCode: number;
    title: string;
    message: string;
    details: string;
    isHtmlEnabled: boolean;
    displayByDefault: boolean;
    showWithToastr: boolean;
    data: T;
    errors: string[];

    constructor(statusCode: number, 
        title: string, 
        message: string, 
        details: string, 
        isHtmlEnabled: boolean = false,
        displayByDefault: boolean = false,
        showWithToastr: boolean = false,
        data: T,
        errors: string[]
    ) {
        this.statusCode = statusCode;
        this.title = title;
        this.message = message;
        this.details = details;
        this.isHtmlEnabled = isHtmlEnabled;
        this.displayByDefault = displayByDefault;
        this.showWithToastr = showWithToastr;
        this.data = data;
        this.errors = errors;
    }
}