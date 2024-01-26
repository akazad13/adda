import { BrowserModule, HammerGestureConfig, HAMMER_GESTURE_CONFIG } from '@angular/platform-browser';
import { NgModule, Injectable } from '@angular/core';
import { HttpClientModule } from '@angular/common/http';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { BsDropdownModule, TabsModule, BsDatepickerModule, PaginationModule, ButtonsModule, ModalModule } from 'ngx-bootstrap';
import { RouterModule } from '@angular/router';
import { NgxGalleryModule } from 'ngx-gallery';
import { JwtModule } from '@auth0/angular-jwt';
import { FileUploadModule } from 'ng2-file-upload';

import { AppComponent } from './app.component';
import { NavComponent } from './nav/nav.component';
import { AuthService } from './services/auth.service';
import { HomeComponent } from './home/home.component';
import { RegisterComponent } from './register/register.component';
import { ErrorInterceptorProvider } from './error-handler/error.interceptor';
import { MemberListComponent } from './members/member-list/member-list.component';
import { ListsComponent } from './lists/lists.component';
import { MessagesComponent } from './messages/messages.component';
import { appRoutes } from './routes';
import { UserService } from './services/user.service';
import { MemberCardComponent } from './members/member-list/member-card/member-card.component';
import { MemberDetailComponent } from './members/member-detail/member-detail.component';
import { MemberDetailResolver } from './resolver/member-detail.resolver';
import { MemberListResolver } from './resolver/member-list.resolver';
import { MemberEditComponent } from './members/member-edit/member-edit.component';
import { MemberEditResolver } from './resolver/member-edit.resolver';
import { PreventUnsavedChanges } from './guards/prevent-unsaved-changes.guard';
import { PhotoEditorComponent } from './members/member-edit/photo-editor/photo-editor.component';
import { ListsResolver } from './resolver/lists.resolver';
import { MessagesResolver } from './resolver/messages';
import { MemberMessagesComponent } from './members/member-detail/member-messages/member-messages.component';
import { AdminPanelComponent } from './admin/admin-panel/admin-panel.component';
import { HasRoleDirective } from './directives/hasRole.directive';
import { UserManagementComponent } from './admin/user-management/user-management.component';
import { PhotoManagementComponent } from './admin/photo-management/photo-management.component';
import { AdminService } from './services/admin.service';
import { RolesModalComponent } from './admin/roles-modal/roles-modal.component';
import { LoderComponent } from './shared/loader.component';
import { LoaderService } from './services/loader.service';

export function tokenGetter() {
  return localStorage.getItem('token');
}
@Injectable()
export class CustomHammerConfig extends HammerGestureConfig {
  overrides = {
    pinch: { enable: false },
    rotate: { enable: false }
  };
}

@NgModule({
    declarations: [
        AppComponent,
        NavComponent,
        HomeComponent,
        RegisterComponent,
        MemberListComponent,
        ListsComponent,
        MessagesComponent,
        MemberCardComponent,
        MemberDetailComponent,
        MemberEditComponent,
        PhotoEditorComponent,
        MemberMessagesComponent,
        AdminPanelComponent,
        HasRoleDirective,
        UserManagementComponent,
        PhotoManagementComponent,
        RolesModalComponent,
        LoderComponent
    ],
    imports: [
        BrowserModule,
        HttpClientModule,
        FormsModule,
        ReactiveFormsModule,
        BrowserAnimationsModule,
        BsDropdownModule.forRoot(),
        BsDatepickerModule.forRoot(),
        TabsModule.forRoot(),
        PaginationModule.forRoot(),
        ButtonsModule.forRoot(),
        RouterModule.forRoot(appRoutes),
        ModalModule.forRoot(),
        NgxGalleryModule,
        FileUploadModule,
        JwtModule.forRoot({
            config: {
                tokenGetter,
                whitelistedDomains: ['aws.akazad.dev'],
                blacklistedRoutes: ['aws.akazad.dev/auth']
            }
        })
    ],
    providers: [
        AuthService,
        UserService,
        ErrorInterceptorProvider,
        ListsResolver,
        MemberDetailResolver,
        MemberListResolver,
        MemberEditResolver,
        MessagesResolver,
        AdminService,
        PreventUnsavedChanges,
        LoaderService,
        { provide: HAMMER_GESTURE_CONFIG, useClass: CustomHammerConfig }
    ],
    bootstrap: [AppComponent]
})
export class AppModule {}
