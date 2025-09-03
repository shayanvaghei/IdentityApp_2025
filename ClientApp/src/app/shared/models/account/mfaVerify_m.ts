export class MfaVerifyModel {
    mfaToken: string;
    code: string;

    constructor(mfaToken: string, code: string) {
        this.mfaToken = mfaToken;
        this.code = code;
    }
}