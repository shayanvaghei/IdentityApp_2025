export interface UserModel {
    name: string;
    jwt: string;
    mfaToken: string;
    roles: string[];
}

export interface AuthStatusModel {
    isAuthenticated: boolean;
}