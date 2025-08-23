import { Routes } from '@angular/router';
import { Home } from './home/home';
import { NotFound } from './shared/components/errors/not-found/not-found';
import { Play } from './play/play';
import { authGuard } from './core/guards/auth-guard';
import { MyProfile } from './my-profile/my-profile';

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
    { path: 'not-found', component: NotFound },
    { path: '**', component: NotFound, pathMatch: 'full' }
];
