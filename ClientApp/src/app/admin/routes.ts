import { Route } from "@angular/router";
import { Admin } from "./admin";
import { AddEditUser } from "./add-edit-user/add-edit-user";

export const accountRoutes: Route[] = [
    { path: '', component: Admin },
    { path: 'add-edit-user', component: AddEditUser },
    { path: 'add-edit-user/:id', component: AddEditUser },
]
