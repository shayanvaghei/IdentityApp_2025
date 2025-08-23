import { EditProfileBaseModel } from "./editProfileBase_m";

export interface ChangePasswordModel extends EditProfileBaseModel {
    newPassword: string;
}