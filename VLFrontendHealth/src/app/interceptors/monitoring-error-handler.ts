import { ErrorHandler, Injector, Injectable } from "@angular/core";

import { ApplicationInsightsService } from "../services/application-insights.service";

@Injectable()
export class MonitoringErrorHandler extends ErrorHandler {
	constructor(private injector: Injector) {
		super();
	}

	handleError(error: any): void {
		const monitoringService = this.injector.get(ApplicationInsightsService);
		monitoringService.logError(error);
		super.handleError(error);
	}
}
