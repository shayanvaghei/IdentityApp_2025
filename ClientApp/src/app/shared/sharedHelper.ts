import { ValidatorFn, AbstractControl } from "@angular/forms";

export function matchValues(matchTo: string): ValidatorFn {
    return (control: AbstractControl) => {
        return control.value === control.parent?.get(matchTo)?.value ? null : { notMatching: true };
    }
}