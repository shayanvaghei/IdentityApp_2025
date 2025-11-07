import { Routes } from '@angular/router';
import { Home } from './home/home';
import { NotFound } from './shared/components/errors/not-found/not-found';
import { Play } from './play/play';
import { authGuard } from './core/guards/auth-guard';
import { MyProfile } from './my-profile/my-profile';
import { roleGuard } from './core/guards/role-guard';

export const routes: Routes = [
    { path: '', component: Home },
    {
        path: '',
        runGuardsAndResolvers: 'always',
        canActivate: [authGuard],
        children: [
            { path: 'play', component: Play },
            { path: 'my-profile/:page', component: MyProfile },
        ]
    },
    { path: 'account', loadChildren: () => import('./account/routes').then(r => r.accountRoutes) },
    {
        path: '',
        runGuardsAndResolvers: 'always',
        canActivate: [roleGuard],
        data: { role: ['admin'] },
        children: [
            { path: 'admin', loadChildren: () => import('./admin/routes').then(module => module.accountRoutes) },
        ]
    },
    {
        path: '',
        runGuardsAndResolvers: 'always',
        canActivate: [roleGuard],
        data: { role: ['admin', 'moderator'] },
        children: [
            { path: 'moderator', loadChildren: () => import('./moderator/routes').then(module => module.accountRoutes) },
        ]
    },
    { path: 'not-found', component: NotFound },
    { path: '**', component: NotFound, pathMatch: 'full' }
];
