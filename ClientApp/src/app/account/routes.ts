import { Route } from "@angular/router";
import { Login } from "./login/login";
import { Register } from "./register/register";
import { ConfirmEmail } from "./confirm-email/confirm-email";
import { SendEmail } from "./send-email/send-email";
import { ResetPassword } from "./reset-password/reset-password";

export const accountRoutes: Route[] = [
    { path: 'login', component: Login },
    { path: 'register', component: Register },
    { path: 'confirm-email', component: ConfirmEmail },
    { path: 'send-email/:mode', component: SendEmail },
    { path: 'reset-password', component: ResetPassword },
]
