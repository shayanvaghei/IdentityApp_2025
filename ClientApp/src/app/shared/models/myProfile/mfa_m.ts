import { EditProfileBaseModel } from "./editProfileBase_m";

export interface QrCodeModel {
    secret: string;
    uri: string;
}

export interface MfaEnableModel extends EditProfileBaseModel {
    secret: string;
    code: string;
}