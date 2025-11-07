export interface UserView {
    userId: number;
    name: string;
    userName: string;
    email: string;
    emailConfirmed: boolean;
    isLocked: boolean;
    isActive: boolean;
    lockoutEnd?: Date;
    createdAt: Date;
    lastActivity: Date;
    csvRoles: string;
    daysToLock: number;
}