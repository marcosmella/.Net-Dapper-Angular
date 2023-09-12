import { Component, OnInit, Input, forwardRef, Output, EventEmitter } from "@angular/core";
import { FormGroup, FormBuilder, ControlValueAccessor, NG_VALUE_ACCESSOR, FormArray, Validators } from "@angular/forms";
import { debounceTime, tap, switchMap, finalize, filter } from "rxjs/operators";

import { Pathologies } from "../../../../models/pathology.model";
import { Select } from "../../../../models/select.model";
import { PathologyService } from "./../../../../services/pathology.service";

@Component({
	selector: "app-pathologies",
	templateUrl: "./pathologies.component.html",
	styleUrls: ["./pathologies.component.scss"],
	providers: [
		{
			provide: NG_VALUE_ACCESSOR,
			useExisting: forwardRef(() => PathologiesComponent),
			multi: true
		}
	]
})
export class PathologiesComponent implements OnInit, ControlValueAccessor {
	@Input("pathologies") set pathologies(value: Array<Pathologies>) {
		if (value.length) {
			this.getPathologies(value);
			this.addPathology = false;
			this.viewFeedback = false;
		} else {
			this.clearPatologies();
			this.viewFeedback = true;
		}
	}

	@Input("disabled") set setDisabled(value: boolean) {
		this.disabled = value;
	}

	@Input("deletePathology") set deletePathology(value: number) {
		if (value) {
			const pathology = new Pathologies();
			pathology.id = value;
			this.formPathologies.removeAt(this.arrayPathologies.length - 1);
			this.arrayPathologies.pop();
		}
	}

	@Output() emitPathologies = new EventEmitter<Array<Pathologies>>();

	public listPathologies: Select[];
	public form: FormGroup;
	public isLoading: boolean;
	public pathologySelected = "";
	public disabled = false;
	public addPathology = true;
	public viewFeedback = false;

	public arrayPathologies = new Array<Pathologies>();
	get formPathologies(): FormArray {
		return <FormArray>this.form.get("formPathologies");
	}

	constructor(private fb: FormBuilder, private patholgyService: PathologyService) {
		this.form = this.fb.group({
			pathologySearch: [null],
			formPathologies: fb.array([])
		});
	}

	onChange = (_: Select) => {};
	onTouch = () => {};

	ngOnInit(): void {
		this.autoComplete();
	}

	createItem(element: Pathologies = null): FormGroup {
		const formItem = this.fb.group({
			id: [element ? (element.id ? Number(element.id) : 0) : null, [Validators.required]],
			description: [element ? (element.description ? element.description : "") : null, [Validators.required]],
			isModify: [element ? (element.isModify ? true : false) : null]
		});
		return formItem;
	}

	writeValue(): void {}

	registerOnChange(fn: any): void {
		this.onChange = fn;
	}

	registerOnTouched(fn: any): void {
		this.onTouch = fn;
	}

	autoComplete(): void {
		const minCharacters = 3;
		this.form
			.get("pathologySearch")
			.valueChanges.pipe(
				filter((x) => x.length >= minCharacters && x.toString() !== this.pathologySelected),
				debounceTime(900),
				tap(() => {
					this.pathologySelected = "";
					this.listPathologies = [];
					this.isLoading = true;
				}),
				switchMap(() => this.patholgyService.get(this.form.value).pipe(finalize(() => (this.isLoading = false))))
			)
			.subscribe(
				(data: Pathologies[]) => {
					const pathologyExists = data.filter((x) => {
						return !this.arrayPathologies.some((element) => element.id === x.id);
					});
					this.listPathologies = pathologyExists;
				},
				() => {
					this.onChange(null);
					this.listPathologies = [];
					this.pathologySelected = "";
					this.autoComplete();
				}
			);

		this.form.get("pathologySearch").valueChanges.subscribe((value) => {
			if (!value) {
				this.onChange(null);
			}
		});
	}

	viewAddBottom(id: number, valid: true): boolean {
		const isView = id === this.formPathologies.value.length - 1 && valid && !this.addPathology && !this.disabled ? true : false;
		return isView;
	}

	removeItem(index: number): void {
		this.arrayPathologies.splice(index, 1);
		this.formPathologies.removeAt(index);
		this.formPathologies.markAsDirty();
		this.emitPathologies.emit(this.arrayPathologies);
		this.form.get("pathologySearch").setValue(null);
		this.viewFeedback = this.formPathologies.length === 0 ? true : false;
	}

	addItem(): void {
		this.addPathology = true;
		const pathology = this.form.get("pathologySearch").value;
		if (!pathology) {
			this.form.get("pathologySearch").setValidators(Validators.required);
		}
	}

	getPathologies(element: Array<Pathologies>): void {
		if (element !== this.arrayPathologies) {
			element.forEach((x) => {
				const isExists = this.arrayPathologies.find((pathology) => pathology.id === x.id) === undefined ? false : true;
				if (!isExists) {
					this.arrayPathologies.push(x);
					this.formPathologies.push(this.createItem(x));
				}
			});
		}
	}

	setPathology(pathology: Pathologies): void {
		pathology.isModify = true;
		pathology.allowSaveInMedicalControl = true;
		const array = new Array<Pathologies>();
		array.push(pathology);
		this.getPathologies(array);
		this.emitPathologies.emit(this.arrayPathologies);
		this.addPathology = false;
		this.form.get("pathologySearch").setValue(null);
	}

	addNewPathology(): void {
		this.addPathology = true;
		this.viewFeedback = false;
	}

	clearPatologies(): void {
		this.arrayPathologies.forEach(() => {
			this.formPathologies.removeAt(0);
			this.addPathology = true;
		});
		this.arrayPathologies = [];
	}
}
