import { Injectable } from "@angular/core";
import { BehaviorSubject } from "rxjs/internal/BehaviorSubject";

import { ApplicationInsightsService } from "./application-insights.service";
import { Tenant } from "../models/tenant.model";
import { UserData } from "../models/user-data.model";
import { Country } from "../models/country.model";

@Injectable({
	providedIn: "root"
})
export class UserService {
	public user: UserData;
	public selectedTenant: Tenant;
	public tenants: Array<Tenant>;
	public accessResource$: BehaviorSubject<any[]> = new BehaviorSubject<any[]>(this.getAccessResource());

	constructor(private applicationInsightsService: ApplicationInsightsService) {
		this.loadUserData();
		this.loadTenant();
		this.loadTenants();
	}

	loadUserData(): UserData {
		this.user = JSON.parse(localStorage.getItem("userData"));
		if (this.user) {
			this.applicationInsightsService.setUserId(this.user.UserId);
		}
		return this.user;
	}

	getTenantCountry(): Country {
		const country = JSON.parse(localStorage.getItem("country"));
		return country;
	}

	loadTenants(): Tenant[] {
		this.tenants = JSON.parse(localStorage.getItem("tenants"));
		return this.tenants;
	}

	loadTenant(): Tenant {
		this.selectedTenant = JSON.parse(localStorage.getItem("tenant"));
		return this.selectedTenant;
	}

	loadToken(): string {
		return localStorage.getItem("token");
	}

	getAccessResource(): any[] {
		const accessResource = JSON.parse(localStorage.getItem("accessResource"));

		return accessResource ? accessResource : [];
	}
}
