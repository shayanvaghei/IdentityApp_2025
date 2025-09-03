import { Route } from "@angular/router";
import { Login } from "./login/login";
import { Register } from "./register/register";
import { ConfirmEmail } from "./confirm-email/confirm-email";
import { SendEmail } from "./send-email/send-email";
import { ResetPassword } from "./reset-password/reset-password";
import { MfaVerify } from "./mfa-verify/mfa-verify";

export const accountRoutes: Route[] = [
    { path: 'login', component: Login },
    { path: 'mfa-verify', component: MfaVerify },
    { path: 'register', component: Register },
    { path: 'confirm-email', component: ConfirmEmail },
    { path: 'send-email/:mode', component: SendEmail },
    { path: 'reset-password', component: ResetPassword },
]
