import { Injectable } from "@angular/core";
import { EnumResources } from "src/assets/enumResources";
import { Router, CanActivate, ActivatedRouteSnapshot } from "@angular/router";

import { AuthService } from "./auth.service";

@Injectable({
	providedIn: "root"
})
export class AuthGuardService implements CanActivate {
	private _resources = EnumResources;
	constructor(public authServ: AuthService, public router: Router) {}

	canActivate(route: ActivatedRouteSnapshot): boolean {
		this.authServ.tokenIsExpired();
		this.authServ.loadUserData();
		this.authServ.loadTenant();
		if (this.authServ.user && this.authServ.selectedTenant) {
			let resources;
			this.authServ.accessResource$.subscribe((value) => (resources = value));

			if (route.data.resource && !resources.find((u) => u.resource.id === this._resources[route.data.resource])) {
				this.router.navigate(["/"]);
				return false;
			}
			return true;
		}
		this.router.navigate(["account/login"]);
		return true;
	}
}
