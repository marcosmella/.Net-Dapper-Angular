import { Component, OnInit, ViewChild } from "@angular/core";
import { FormBuilder, FormGroup, Validators } from "@angular/forms";
import { Observable } from "rxjs/internal/Observable";

import { MedicalControlsService } from "../../../../../services/medical-controls.service";
import { Select } from "./../../../../../models/select.model";
import { LoadingSpinnerComponent } from "../../../../../components/loading-spinner/loading-spinner.component";
import { TranslatePipe } from "../../../../../pipes/translate.pipe";

@Component({
	selector: "app-action-virtual-attention",
	templateUrl: "./action-virtual-attention.component.html",
	styleUrls: ["./action-virtual-attention.component.scss"]
})
export class ActionVirtualAttentionComponent implements OnInit {
	@ViewChild("virtualAttentionSpinner", { static: false })
	public virtualAttentionSpinner: LoadingSpinnerComponent;
	public formVirtualAttentionMedicalControl: FormGroup;
	public form$: Observable<FormGroup>;
	public testResult: Select[];
	public editing = false;
	public today = new Date().At0Time();
	constructor(private fb: FormBuilder, private medicalControlService: MedicalControlsService, private translatePipe: TranslatePipe) {
		this.formVirtualAttentionMedicalControl = this.fb.group({
			testDate: [null, Validators.required],
			testResult: [null, Validators.required]
		});
		this.medicalControlService.actionForm = this.formVirtualAttentionMedicalControl;
		this.testResult = [
			{ id: 0, description: `${this.translatePipe.transform("notDetectable")}` },
			{ id: 1, description: `${this.translatePipe.transform("detectable")}` }
		];
	}

	ngOnInit(): void {
		this.form$ = this.medicalControlService.getActionForm$();
		this.form$.subscribe((form) => {
			this.medicalControlService.actionForm = form;
			if (form.get("testDate").value) {
				this.editing = true;
				this.formVirtualAttentionMedicalControl.get("testDate").disable();
				this.formVirtualAttentionMedicalControl.get("testDate").setValue(form.get("testDate").value);
				const testResult = form.get("testResult").value === true ? 1 : 0;
				this.formVirtualAttentionMedicalControl.get("testResult").setValue(testResult);
			}
		});
	}

	setMedicalControlVirtualAttention(): void {
		this.medicalControlService.medicalControlData.testDate = this.formVirtualAttentionMedicalControl.get("testDate").value;
		this.medicalControlService.medicalControlData.testResult = this.formVirtualAttentionMedicalControl.get("testResult").value;
	}
}
