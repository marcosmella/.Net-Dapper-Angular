import { Injectable, APP_INITIALIZER } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { BehaviorSubject } from "rxjs/internal/BehaviorSubject";
import { Observable } from "rxjs/internal/Observable";

@Injectable({
	providedIn: "root"
})
export class AppConfigService {
	public load = true;
	public errorConf = "";
	private appConfig: any;
	private loadObs: BehaviorSubject<boolean>;

	constructor(private http: HttpClient) {
		this.loadObs = new BehaviorSubject<boolean>(this.load);
	}

	loadAppConfig(): Promise<unknown> {
		const file = `/assets/config.json`;
		return new Promise((resolve, reject) => {
			this.http.get(file).subscribe(
				(data) => {
					this.appConfig = data;
					this.load = false;
					this.changeLoading(this.load);
					resolve(true);
				},
				(error) => {
					this.errorConf = error;
					reject(error);
				}
			);
		});
	}

	get ocpApiSubscriptionKey(): string {
		if (!this.appConfig) {
			throw Error("Config file not loaded!");
		}
		return this.appConfig.ocpApiSubscriptionKey;
	}
	get googleMapsApiKey(): any {
		if (!this.appConfig) {
			return;
		}
		return this.appConfig.googleMapsApiKey;
	}
	get hotjarId(): any {
		if (!this.appConfig) {
			return;
		}
		return this.appConfig.hotjarId;
	}

	public loading(): Observable<boolean> {
		return this.loadObs.asObservable();
	}
	get instrumentationKey(): string {
		if (!this.appConfig) {
			return;
		}
		return this.appConfig.instrumentationKey;
	}
	get googleAnalyticsKey(): string {
		if (!this.appConfig) {
			return;
		}
		return this.appConfig.googleAnalyticsKey;
	}
	get googleAnalyticsKeyGTM(): string {
		if (!this.appConfig) {
			return;
		}
		return this.appConfig.googleAnalyticsKeyGTM;
	}

	get adminEndpoint(): string {
		if (!this.appConfig) {
		}
		return this.appConfig.apiEndpointAdmin;
	}
	get webapiEndpoint(): string {
		if (!this.appConfig) {
			throw Error("Config file not loaded!");
		}
		return this.appConfig.apiEndpointWebApi;
	}
	get apiEndpoint(): string {
		if (!this.appConfig) {
			throw Error("Config file not loaded!");
		}
		return this.appConfig.apiEndpoint;
	}
	get rhProURL(): string {
		if (!this.appConfig) {
			throw Error("Config file not loaded!");
		}
		return this.appConfig.rhProURL;
	}
	private changeLoading(newValue: boolean): void {
		this.loadObs.next(newValue);
	}
}

export function ConfigFactory(config: AppConfigService): Function {
	return () => config.loadAppConfig();
}

export function init(): any {
	return {
		provide: APP_INITIALIZER,
		useFactory: ConfigFactory,
		deps: [AppConfigService],
		multi: true
	};
}

const ConfigModule = {
	init: init
};

export { ConfigModule };
