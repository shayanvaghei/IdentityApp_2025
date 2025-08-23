import { EditProfileBaseModel } from "./editProfileBase_m";

export interface DeleteAccountModel extends EditProfileBaseModel {
    currentUserName: string;
    confirmation: boolean;
}