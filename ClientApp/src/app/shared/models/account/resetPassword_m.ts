export class ResetPasswordModel {
    token: string;
    email: string;
    newPassword: string;

    constructor(token: string, email: string, newPassword: string) {
        this.token = token;
        this.email = email;
        this.newPassword = newPassword;
    }
}