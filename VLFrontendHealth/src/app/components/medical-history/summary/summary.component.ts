import { Component, Input, OnInit, ViewChild } from "@angular/core";

import { EmployeeDetailService } from "../../../services/employee-detail.service";
import { UserPreference } from "../../../models/user-preference.model";
import { AlertService } from "../../../services/alert.service";
import { LoadingSpinnerComponent } from "../../loading-spinner/loading-spinner.component";
import { EmployeeAttributeService } from "../../../services/employee-attribute.service";
import { EmployeeAttributes } from "./../../../models/employee-attributes.model";

@Component({
	selector: "app-summary",
	templateUrl: "./summary.component.html",
	styleUrls: ["./summary.component.scss"]
})
export class SummaryComponent implements OnInit {
	@ViewChild("spinnerSummary", { static: false })
	spinnerSummary: LoadingSpinnerComponent;
	@Input("idEmployee") set enabledEmployee(value: number) {
		if (value) {
			this.idEmployee = value;
			this.setEmployeeAttributes();
		}
	}
	@Input() editing: boolean;

	idTypeEmployee = 1;
	idFileEmployee = 3;
	public idImage = "employee";
	public employeeAttributes: Array<EmployeeAttributes> = [];
	public employeeHealthInsurance = "";
	public employeeHealthInsurancePlan = "";
	public employeeCenterCost = "";
	public idEmployee: number;
	constructor(
		private alertService: AlertService,
		private employeeAttributeService: EmployeeAttributeService,
		public employeeDetailService: EmployeeDetailService,
		public userPreference: UserPreference
	) {}

	ngOnInit(): void {}

	errorImage(message: string[]): void {
		this.alertService.error(message, "medicalHealthError");
	}

	setEmployeeAttributes(): void {
		this.employeeAttributeService.getEmployeeAttributes(this.idEmployee).subscribe(
			(data) => {
				const healthInsuranceStructureType = 17;
				const centerCostStructureType = 5;
				const healthInsurancePlanStructureType = 23;
				this.employeeHealthInsurance = this.getAttributeDescription(data, healthInsuranceStructureType);
				this.employeeHealthInsurancePlan = this.getAttributeDescription(data, healthInsurancePlanStructureType);
				this.employeeCenterCost = this.getAttributeDescription(data, centerCostStructureType);
			},
			(error) => {
				if (error.status !== 404) {
					this.alertService.error(error.error.Error, "medicalHealthError");
				}
			}
		);
	}

	getAttributeDescription(data: any, idStructure: number): string {
		this.employeeAttributes = data.filter((x) => {
			return x.structure.structureType.id === idStructure && x.structure.active === true;
		});
		return this.employeeAttributes[0].structure.description;
	}
}
