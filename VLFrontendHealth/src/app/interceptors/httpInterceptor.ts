import { Injectable } from "@angular/core";
import { HttpEvent, HttpInterceptor, HttpHandler, HttpRequest, HttpHeaders } from "@angular/common/http";
import { Observable } from "rxjs/internal/Observable";

import { AppConfigService } from "../services/app.config.service";
import { UserService } from "../services/user.service";

@Injectable({
	providedIn: "root"
})
export class MyHttpInterceptor implements HttpInterceptor {
	constructor(public userService: UserService, private conf: AppConfigService) {}

	intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
		// Clone the request to add the new header.
		let authReq;
		if (this.userService.loadToken()) {
			const headers = {
				"Content-Type": "application/json"
			};
			headers["Ocp-Apim-Subscription-Key"] = !this.conf.load ? this.conf.ocpApiSubscriptionKey : "";
			headers["Access-Control-Allow-Origin"] = "*";
			headers["Authorization"] = `Bearer ${this.userService.loadToken()}`;
			const tenant = this.userService.loadTenant();
			if (tenant) {
				headers["X-Tenant-Id"] = this.userService.loadTenant().Id;
			}

			authReq = req.clone({
				headers: new HttpHeaders(headers)
			});
		} else {
			authReq = req.clone({
				headers: new HttpHeaders({
					"Ocp-Apim-Subscription-Key": !this.conf.load ? this.conf.ocpApiSubscriptionKey : ""
				})
			});
		}

		return next.handle(authReq);
	}
}
