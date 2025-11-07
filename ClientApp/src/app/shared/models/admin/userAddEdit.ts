export interface UserAddEdit {
    userId: string;
    name: string;
    email: string;
    password?: string;
    emailConfirmed: boolean;
    isActive: boolean;
    lastActivity: Date;
    roles: string[];
}