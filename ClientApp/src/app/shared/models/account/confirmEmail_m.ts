export class ConfirmEmailModel {
    token: string;
    email: string;

    constructor(token: string, email: string) {
        this.token = token;
        this.email = email;
    }
}

export class EmailModel {
    email: string;

    constructor(email: string) {
        this.email = email;
    }
}