import { Input } from "@angular/core";
import { AfterViewInit, Component, OnInit } from "@angular/core";
import { ActivatedRoute, Params, Router } from "@angular/router";

import { ActionRoute } from "../../../../models/action-route.model";
import { MedicalControlsService } from "../../../../services/medical-controls.service";
import { MedicalControlActionConfig } from "../../../../models/medical-control-action-config.model";

@Component({
	selector: "app-actions",
	templateUrl: "./actions.component.html",
	styleUrls: ["./actions.component.scss"]
})
export class ActionsComponent implements OnInit, AfterViewInit {
	@Input("queryParams") set queryParameteres(value: ActivatedRoute) {
		if (value) {
			value.queryParamMap.subscribe((queryParams) => {
				this.queryParams = queryParams;
			});
		}
	}
	@Input("action") set idAction(value: number) {
		if (value >= 0) {
			this.setActionSchema(value);
		}
	}
	public medicalControlActionConfig = new MedicalControlActionConfig();
	public queryParams: Params;
	constructor(private router: Router, private medicalControlService: MedicalControlsService) {}

	ngOnInit(): void {}

	ngAfterViewInit(): void {}

	setActionSchema(id: number): void {
		this.medicalControlActionConfig.getActionById(id, this.medicalControlService.medicalControlData.id).then((data: ActionRoute) => {
			if (data.route) {
				this.router.navigateByUrl(this.router.createUrlTree([data.route], { queryParams: this.queryParams?.params }));
			}
		});
	}
}
