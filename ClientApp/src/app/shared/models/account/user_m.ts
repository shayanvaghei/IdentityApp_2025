export interface UserModel {
    name: string;
    jwt: string;
    mfaToken: string;
}

export interface AuthStatusModel {
    isAuthenticated: boolean;
}