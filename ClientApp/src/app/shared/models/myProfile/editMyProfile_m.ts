import { EditProfileBaseModel } from "./editProfileBase_m";

export interface EditMyProfileModel extends EditProfileBaseModel {
    name: string;
    email: string;
}