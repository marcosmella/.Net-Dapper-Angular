import { MatDialog } from "@angular/material/dialog";
import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs/internal/Observable";
import { BehaviorSubject } from "rxjs/internal/BehaviorSubject";
import * as moment from "moment";

import { AppConfigService } from "./app.config.service";
import { ApplicationInsightsService } from "./application-insights.service";
import { Tenant } from "../models/tenant.model";
import { UserData } from "./../models/user-data.model";
import { Country } from "../models/country.model";
import { ModalComponent } from "../components/modal/modal.component";
@Injectable({
	providedIn: "root"
})
export class AuthService {
	public user: UserData;
	public selectedTenant: Tenant;
	public tenants: Array<Tenant>;
	public loggedInAndTenantSelected$: Observable<boolean>;
	public accessResource$: BehaviorSubject<any[]> = new BehaviorSubject<any[]>(this.getAccessResource());
	private loggedInAndTenantSelectedSubject$ = new BehaviorSubject<boolean>(false);

	constructor(
		public dialog: MatDialog,
		private http: HttpClient,
		private conf: AppConfigService,
		private applicationInsightsService: ApplicationInsightsService
	) {
		this.loggedInAndTenantSelected$ = this.loggedInAndTenantSelectedSubject$.asObservable();
		this.loadUserData();
		this.loadTenant();
		this.loadTenants();
		if (this.user && this.selectedTenant) {
			this.loggedInAndTenantSelectedSubject$.next(true);
		}
	}

	login(credentials: any): Observable<any> {
		return this.http.post(`${this.conf.apiEndpoint}/WebApiAdmin/authentication/login`, credentials);
	}

	loadUserData(): UserData {
		this.user = JSON.parse(localStorage.getItem("userData"));
		if (this.user) {
			this.applicationInsightsService.setUserId(this.user.UserId);
		}
		return this.user;
	}

	getTenants(): Observable<Tenant[]> {
		return this.http.get<Tenant[]>(`${this.conf.apiEndpoint}/WebApiAdmin/account/tenants`);
	}

	setTenant(tenant: Tenant): Promise<any> {
		this.selectedTenant = tenant;
		localStorage.setItem("tenant", JSON.stringify(tenant));
		return this.setTenantCountry();
	}
	getTenantCountry(): Country {
		const country = JSON.parse(localStorage.getItem("country"));
		if (!country) {
			this.setTenantCountry();
		}
		return country;
	}

	setTenants(tenants: Tenant[]): void {
		this.tenants = tenants;
		localStorage.setItem("tenants", JSON.stringify(tenants));
	}

	loadTenants(): Tenant[] {
		this.tenants = JSON.parse(localStorage.getItem("tenants"));
		return this.tenants;
	}

	setUserData(userData: UserData): void {
		this.user = userData;
		this.applicationInsightsService.setUserId(userData.UserId);
		localStorage.setItem("userData", JSON.stringify(userData));
	}

	getUserInfo(): Observable<UserData> {
		return this.http.get<UserData>(`${this.conf.apiEndpoint}/WebApiAdmin/account/user-info`);
	}

	loadTenant(): Tenant {
		this.selectedTenant = JSON.parse(localStorage.getItem("tenant"));
		return this.selectedTenant;
	}

	tokenIsExpired(): void {
		const token = JSON.parse(localStorage.getItem("expiresToken"));
		if (token && token.date) {
			const tokenTime = moment(token.date);
			const timeSinceLastLogin = moment(new Date()).diff(tokenTime, "seconds");

			if (timeSinceLastLogin >= token.expires) {
				this.user = null;
				this.tenants = null;
				this.logout();
			}
		}
	}

	logout(): void {
		this.loggedInAndTenantSelectedSubject$.next(false);
		this.http.post(`${this.conf.apiEndpoint}/WebApiAdmin/authentication/logout`, {});
		this.applicationInsightsService.clearUserId();

		this.clearSessionStorage();
	}

	loadToken(): string {
		return localStorage.getItem("token");
	}
	setToken(token: string): void {
		localStorage.setItem("token", token);
	}

	setUser(idUserBinding: string): void {
		localStorage.setItem("idUserBinding", idUserBinding);
	}

	setAccessResource(data: any): void {
		localStorage.setItem("accessResource", JSON.stringify(data));
		this.accessResource$.next(data);
		this.loggedInAndTenantSelectedSubject$.next(true);
	}

	getAccessResource(): any[] {
		const accessResource = JSON.parse(localStorage.getItem("accessResource"));
		return accessResource ? accessResource : [];
	}

	private setTenantCountry(): Promise<any> {
		return new Promise((resolve, reject) => {
			this.http.get<Country[]>(`${this.conf.apiEndpoint}Geolocation/api/countries?ISO=${this.selectedTenant.Country}`).subscribe(
				(data) => {
					localStorage.setItem("country", JSON.stringify(data[0]));
					return resolve("");
				},
				(error) => {
					const errorCustom = error.status === 401 ? error.error : error.error.Error;

					this.dialog.open(ModalComponent, {
						disableClose: true,
						hasBackdrop: true,
						width: "540px",
						height: "auto",
						data: {
							message: errorCustom,
							title: "error"
						}
					});

					return reject("");
				}
			);
		});
	}

	private clearSessionStorage(): void {
		localStorage.removeItem("userData");
		localStorage.removeItem("token");
		localStorage.removeItem("tenant");
		localStorage.removeItem("tenants");
		localStorage.removeItem("accessResource");
		localStorage.removeItem("country");
		localStorage.removeItem("userPreferences");
		localStorage.removeItem("notifications");
		localStorage.removeItem("notificationsList");
		localStorage.removeItem("selectedDomain");
		localStorage.removeItem("adal.idtoken");
	}
}
