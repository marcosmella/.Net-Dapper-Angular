import { Component, OnInit, ViewChild } from "@angular/core";
import { FormBuilder, FormGroup, Validators } from "@angular/forms";
import { Observable } from "rxjs/internal/Observable";

import { TranslatePipe } from "./../../../../../pipes/translate.pipe";
import { LoadingSpinnerComponent } from "../../../../../components/loading-spinner/loading-spinner.component";
import { MedicalControlsService } from "../../../../../services/medical-controls.service";

@Component({
	selector: "app-action-break",
	templateUrl: "./action-break.component.html",
	styleUrls: ["./action-break.component.scss"]
})
export class ActionBreakComponent implements OnInit {
	@ViewChild("breakSpinner", { static: false })
	public breakSpinner: LoadingSpinnerComponent;
	public breakTimes: Array<string>;
	public formBreakMedicalControl: FormGroup;
	public form$: Observable<FormGroup>;
	public editing = false;
	constructor(private fb: FormBuilder, private medicalControlService: MedicalControlsService, private translatePipe: TranslatePipe) {
		this.formBreakMedicalControl = this.fb.group({
			breakTime: [null, Validators.required]
		});
		this.medicalControlService.actionForm = this.formBreakMedicalControl;
	}

	ngOnInit(): void {
		this.setBreakTimes();
		this.form$ = this.medicalControlService.getActionForm$();
		this.form$.subscribe((form) => {
			this.medicalControlService.actionForm = form;
			if (form.get("breakTime")) {
				this.editing = true;
				this.formBreakMedicalControl.get("breakTime").setValue(form.get("breakTime").value);
			}
		});
	}

	setBreakTimes(): void {
		this.breakTimes = new Array();
		const minutes = this.translatePipe.transform("minutes");
		const maxIntervalsInMinutesPerHour = 12;
		const fiveMinutes = 5;
		for (let x = 1; x < maxIntervalsInMinutesPerHour; x++) {
			this.breakTimes.push(`${x * fiveMinutes} ${minutes}`);
		}
	}

	setMedicalControlBreak(): void {
		const breakTime = this.formBreakMedicalControl.get("breakTime").value;
		this.medicalControlService.medicalControlData.breakTime = breakTime.replace(` ${this.translatePipe.transform("minutes")}`, "");
	}
}
