import { Injectable } from "@angular/core";
import { ApplicationInsights, SeverityLevel } from "@microsoft/applicationinsights-web";
import { ActivatedRouteSnapshot, ResolveEnd, Router } from "@angular/router";
import { filter } from "rxjs/operators";
import { Subscription } from "rxjs/internal/Subscription";

import { AppConfigService } from "./app.config.service";

@Injectable({
	providedIn: "root"
})
export class ApplicationInsightsService {
	public routerSubscription: Subscription;
	private appInsights: ApplicationInsights;
	private queque: Array<any> = [];

	constructor(private router: Router, private conf: AppConfigService) {
		this.conf.loading().subscribe(load => {
			if (!load) {
				this.appInsights = new ApplicationInsights({
					config: {
						instrumentationKey: conf.instrumentationKey
					}
				});
				this.appInsights.loadAppInsights();
				this.queque.forEach(item => {
					this.appInsights[item.method](item.params);
				});
				this.queque = [];
				this.routerSubscription = this.router.events
					.pipe(filter(event => event instanceof ResolveEnd))
					.subscribe((event: ResolveEnd) => {
						const activatedComponent = this.getActivatedComponent(event.state.root);
						if (activatedComponent) {
							this.logPageView(
								`${activatedComponent.name} ${this.getRouteTemplate(event.state.root)}`,
								event.urlAfterRedirects
							);
						}
					});
			}
		});
	}

	setUserId(userId: string): void {
		if (!this.appInsights) {
			this.queque.push({
				method: "setAuthenticatedUserContext",
				params: userId
			});
			return;
		}
		this.appInsights.setAuthenticatedUserContext(userId);
	}

	clearUserId(): void {
		if (!this.appInsights) {
			this.queque.push({
				method: "clearAuthenticatedUserContext",
				params: null
			});
			return;
		}
		this.appInsights.clearAuthenticatedUserContext();
	}

	logPageView(name?: string, uri?: string): void {
		if (!this.appInsights) {
			this.queque.push({
				method: "trackPageView",
				params: { name, uri }
			});
			return;
		}
		this.appInsights.trackPageView({ name, uri });
	}

	public logEvent(): void {
		if (!this.appInsights) {
			this.queque.push({
				method: "trackTrace",
				params: {
					message: `App initialised at ${new Date().toString()}`,
					severityLevel: SeverityLevel.Information
				}
			});
			return;
		}
		this.appInsights.trackTrace({
			message: `App initialised at ${new Date().toString()}`,
			severityLevel: SeverityLevel.Information
		});
	}

	public logError(error: Error): void {
		if (!this.appInsights) {
			this.queque.push({
				method: "trackException",
				param: { error }
			});
			return;
		}
		this.appInsights.trackException({ error });
	}

	private getActivatedComponent(snapshot: ActivatedRouteSnapshot): any {
		if (snapshot.firstChild) {
			return this.getActivatedComponent(snapshot.firstChild);
		}

		return snapshot.component;
	}

	private getRouteTemplate(snapshot: ActivatedRouteSnapshot): string {
		let path = "";
		if (snapshot.routeConfig) {
			path += snapshot.routeConfig.path;
		}

		if (snapshot.firstChild) {
			return path + this.getRouteTemplate(snapshot.firstChild);
		}

		return path;
	}
}
