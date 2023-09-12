import { Directive, ElementRef, HostListener } from "@angular/core";

@Directive({
	// tslint:disable-next-line: directive-selector
	selector: "[form-validation]"
})
export class FormValidationDirective {
	constructor(private el: ElementRef) {}

	@HostListener("submit", ["$event"]) onEvent(): void {
		const inputs = this.el.nativeElement.querySelectorAll("input");
		inputs.forEach((element) => {
			element.focus();
			element.blur();
		});
	}
}
