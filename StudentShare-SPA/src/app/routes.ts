import {Routes} from '@angular/router';
import { HomeComponent } from './home/home.component';
import { MemberListComponent } from './members/member-list/member-list.component';
import { MessagesComponent } from './messages/messages.component';
import { ListsComponent } from './lists/lists.component';
import { AuthGuard } from './_guards/auth.guard';
import { MemberDetailComponent } from './members/member-detail/member-detail.component';
import { MemberDetailResolver } from './_resolvers/member-detail.resolver';
import { MemberListResolver } from './_resolvers/member-list.resolver';


// routes direct each link - authguard protects them against users that are not currently
// logged in

// these routes work in a first match wins basis, so the wildcard at the end
// automatically redirects to the home page if no other route is found
export const appRoutes: Routes = [
    {path: '', component: HomeComponent},
    {path: 'members', component: MemberListComponent, resolve: {users: MemberListResolver},
    canActivate: [AuthGuard]},
    {path: 'members/:id', component: MemberDetailComponent,
    resolve: {user: MemberDetailResolver}, canActivate: [AuthGuard]},
    {path: 'messages', component: MessagesComponent, canActivate: [AuthGuard]},
    {path: 'lists', component: ListsComponent, canActivate: [AuthGuard]},
    {path: '**', redirectTo: '', pathMatch: 'full'},
];
